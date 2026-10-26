//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quick_estimate_list_item_response.g.dart';

/// QuickEstimateListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [status] 
/// * [roomOrArea] 
/// * [templateCode] 
/// * [displayedLower] 
/// * [displayedUpper] 
/// * [shareDecision] 
/// * [updatedAtUtc] 
@BuiltValue()
abstract class QuickEstimateListItemResponse implements Built<QuickEstimateListItemResponse, QuickEstimateListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'roomOrArea')
  String? get roomOrArea;

  @BuiltValueField(wireName: r'templateCode')
  String? get templateCode;

  @BuiltValueField(wireName: r'displayedLower')
  double? get displayedLower;

  @BuiltValueField(wireName: r'displayedUpper')
  double? get displayedUpper;

  @BuiltValueField(wireName: r'shareDecision')
  String? get shareDecision;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  QuickEstimateListItemResponse._();

  factory QuickEstimateListItemResponse([void updates(QuickEstimateListItemResponseBuilder b)]) = _$QuickEstimateListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuickEstimateListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuickEstimateListItemResponse> get serializer => _$QuickEstimateListItemResponseSerializer();
}

class _$QuickEstimateListItemResponseSerializer implements PrimitiveSerializer<QuickEstimateListItemResponse> {
  @override
  final Iterable<Type> types = const [QuickEstimateListItemResponse, _$QuickEstimateListItemResponse];

  @override
  final String wireName = r'QuickEstimateListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuickEstimateListItemResponse object, {
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
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.roomOrArea != null) {
      yield r'roomOrArea';
      yield serializers.serialize(
        object.roomOrArea,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.templateCode != null) {
      yield r'templateCode';
      yield serializers.serialize(
        object.templateCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayedLower != null) {
      yield r'displayedLower';
      yield serializers.serialize(
        object.displayedLower,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.displayedUpper != null) {
      yield r'displayedUpper';
      yield serializers.serialize(
        object.displayedUpper,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.shareDecision != null) {
      yield r'shareDecision';
      yield serializers.serialize(
        object.shareDecision,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuickEstimateListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuickEstimateListItemResponseBuilder result,
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
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'roomOrArea':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roomOrArea = valueDes;
          break;
        case r'templateCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.templateCode = valueDes;
          break;
        case r'displayedLower':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.displayedLower = valueDes;
          break;
        case r'displayedUpper':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.displayedUpper = valueDes;
          break;
        case r'shareDecision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.shareDecision = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuickEstimateListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuickEstimateListItemResponseBuilder();
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


