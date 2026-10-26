//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'checklist_item_request.g.dart';

/// ChecklistItemRequest
///
/// Properties:
/// * [title] 
/// * [required_] 
@BuiltValue()
abstract class ChecklistItemRequest implements Built<ChecklistItemRequest, ChecklistItemRequestBuilder> {
  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'required')
  bool? get required_;

  ChecklistItemRequest._();

  factory ChecklistItemRequest([void updates(ChecklistItemRequestBuilder b)]) = _$ChecklistItemRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ChecklistItemRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ChecklistItemRequest> get serializer => _$ChecklistItemRequestSerializer();
}

class _$ChecklistItemRequestSerializer implements PrimitiveSerializer<ChecklistItemRequest> {
  @override
  final Iterable<Type> types = const [ChecklistItemRequest, _$ChecklistItemRequest];

  @override
  final String wireName = r'ChecklistItemRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ChecklistItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
  }

  @override
  Object serialize(
    Serializers serializers,
    ChecklistItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ChecklistItemRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ChecklistItemRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ChecklistItemRequestBuilder();
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


