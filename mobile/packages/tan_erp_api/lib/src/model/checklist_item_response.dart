//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/service_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'checklist_item_response.g.dart';

/// ChecklistItemResponse
///
/// Properties:
/// * [id] 
/// * [sortOrder] 
/// * [title] 
/// * [required_] 
/// * [done] 
/// * [doneBy] 
/// * [doneAtUtc] 
@BuiltValue()
abstract class ChecklistItemResponse implements Built<ChecklistItemResponse, ChecklistItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'required')
  bool? get required_;

  @BuiltValueField(wireName: r'done')
  bool? get done;

  @BuiltValueField(wireName: r'doneBy')
  ServicePersonResponse? get doneBy;

  @BuiltValueField(wireName: r'doneAtUtc')
  DateTime? get doneAtUtc;

  ChecklistItemResponse._();

  factory ChecklistItemResponse([void updates(ChecklistItemResponseBuilder b)]) = _$ChecklistItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ChecklistItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ChecklistItemResponse> get serializer => _$ChecklistItemResponseSerializer();
}

class _$ChecklistItemResponseSerializer implements PrimitiveSerializer<ChecklistItemResponse> {
  @override
  final Iterable<Type> types = const [ChecklistItemResponse, _$ChecklistItemResponse];

  @override
  final String wireName = r'ChecklistItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ChecklistItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.title != null) {
      yield r'title';
      yield serializers.serialize(
        object.title,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.required_ != null) {
      yield r'required';
      yield serializers.serialize(
        object.required_,
        specifiedType: const FullType(bool),
      );
    }
    if (object.done != null) {
      yield r'done';
      yield serializers.serialize(
        object.done,
        specifiedType: const FullType(bool),
      );
    }
    if (object.doneBy != null) {
      yield r'doneBy';
      yield serializers.serialize(
        object.doneBy,
        specifiedType: const FullType(ServicePersonResponse),
      );
    }
    if (object.doneAtUtc != null) {
      yield r'doneAtUtc';
      yield serializers.serialize(
        object.doneAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ChecklistItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ChecklistItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'title':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.title = valueDes;
          break;
        case r'required':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.required_ = valueDes;
          break;
        case r'done':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.done = valueDes;
          break;
        case r'doneBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.doneBy.replace(valueDes);
          break;
        case r'doneAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.doneAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ChecklistItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ChecklistItemResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


