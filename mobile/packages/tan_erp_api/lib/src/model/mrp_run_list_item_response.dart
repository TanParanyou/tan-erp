//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_run_list_item_response.g.dart';

/// MrpRunListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [asOfDate] 
/// * [recommendationCount] 
/// * [shortageCount] 
/// * [openCount] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class MrpRunListItemResponse implements Built<MrpRunListItemResponse, MrpRunListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'asOfDate')
  Date? get asOfDate;

  @BuiltValueField(wireName: r'recommendationCount')
  int? get recommendationCount;

  @BuiltValueField(wireName: r'shortageCount')
  int? get shortageCount;

  @BuiltValueField(wireName: r'openCount')
  int? get openCount;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  MrpRunListItemResponse._();

  factory MrpRunListItemResponse([void updates(MrpRunListItemResponseBuilder b)]) = _$MrpRunListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpRunListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpRunListItemResponse> get serializer => _$MrpRunListItemResponseSerializer();
}

class _$MrpRunListItemResponseSerializer implements PrimitiveSerializer<MrpRunListItemResponse> {
  @override
  final Iterable<Type> types = const [MrpRunListItemResponse, _$MrpRunListItemResponse];

  @override
  final String wireName = r'MrpRunListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpRunListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.asOfDate != null) {
      yield r'asOfDate';
      yield serializers.serialize(
        object.asOfDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.recommendationCount != null) {
      yield r'recommendationCount';
      yield serializers.serialize(
        object.recommendationCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.shortageCount != null) {
      yield r'shortageCount';
      yield serializers.serialize(
        object.shortageCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.openCount != null) {
      yield r'openCount';
      yield serializers.serialize(
        object.openCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpRunListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpRunListItemResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'asOfDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.asOfDate = valueDes;
          break;
        case r'recommendationCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.recommendationCount = valueDes;
          break;
        case r'shortageCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.shortageCount = valueDes;
          break;
        case r'openCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.openCount = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpRunListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpRunListItemResponseBuilder();
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


