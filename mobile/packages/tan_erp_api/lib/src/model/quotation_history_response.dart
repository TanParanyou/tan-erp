//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/quotation_history_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_history_response.g.dart';

/// QuotationHistoryResponse
///
/// Properties:
/// * [estimateId] 
/// * [items] 
@BuiltValue()
abstract class QuotationHistoryResponse implements Built<QuotationHistoryResponse, QuotationHistoryResponseBuilder> {
  @BuiltValueField(wireName: r'estimateId')
  String? get estimateId;

  @BuiltValueField(wireName: r'items')
  BuiltList<QuotationHistoryItemResponse>? get items;

  QuotationHistoryResponse._();

  factory QuotationHistoryResponse([void updates(QuotationHistoryResponseBuilder b)]) = _$QuotationHistoryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationHistoryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationHistoryResponse> get serializer => _$QuotationHistoryResponseSerializer();
}

class _$QuotationHistoryResponseSerializer implements PrimitiveSerializer<QuotationHistoryResponse> {
  @override
  final Iterable<Type> types = const [QuotationHistoryResponse, _$QuotationHistoryResponse];

  @override
  final String wireName = r'QuotationHistoryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationHistoryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.estimateId != null) {
      yield r'estimateId';
      yield serializers.serialize(
        object.estimateId,
        specifiedType: const FullType(String),
      );
    }
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationHistoryItemResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationHistoryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationHistoryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'estimateId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateId = valueDes;
          break;
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationHistoryItemResponse)]),
          ) as BuiltList<QuotationHistoryItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationHistoryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationHistoryResponseBuilder();
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


